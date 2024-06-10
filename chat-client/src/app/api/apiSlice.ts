import {
  BaseQueryApi,
  BaseQueryFn,
  createApi,
  FetchArgs,
  fetchBaseQuery,
  FetchBaseQueryError,
} from "@reduxjs/toolkit/query/react"
import { setCredentials, logOut } from "../../features/auth/authSlice"
import { RootState, AppDispatch } from "../store" // Assuming you have these types defined
import { Mutex } from "async-mutex"

const apiUrl = import.meta.env.VITE_SERVER_URL as string

const baseQuery = fetchBaseQuery({
  baseUrl: apiUrl + "api",
  credentials: "include",
  prepareHeaders: (headers, { getState }) => {
    headers.set("content-type", "application/json")
    const token = (getState() as RootState).auth.token
    if (token) {
      headers.set("authorization", `Bearer ${token}`)
    }
    return headers
  },
})

const mutex = new Mutex()
const baseQueryWithReauth = async (
  args: string | FetchArgs,
  api: BaseQueryApi,
  extraOptions: {},
) => {
  await mutex.waitForUnlock()

  let result = await baseQuery(args, api, extraOptions)

  if (result?.error && (result.error as FetchBaseQueryError).status === 401) {
    console.log("401 error, refreshing token")

    // Lock the mutex to prevent multiple token refreshes
    if (!mutex.isLocked()) {
      const release = await mutex.acquire()
      try {
        const refreshResult = await baseQuery(
          { url: "/token/refresh", method: "POST" },
          api,
          extraOptions,
        )

        if (refreshResult?.data) {
          const refreshData = refreshResult.data as TokenResponse
          ;(api.dispatch as AppDispatch)(
            setCredentials({
              accessToken: refreshData.accessToken,
            }),
          )

          // Retry the original query with new token
          result = await baseQuery(args, api, extraOptions)
        } else {
          ;(api.dispatch as AppDispatch)(logOut())
        }
      } finally {
        release()
      }
    } else {
      await mutex.waitForUnlock()
      result = await baseQuery(args, api, extraOptions)
    }
  }

  return result
}

export const apiSlice = createApi({
  baseQuery: baseQueryWithReauth,
  endpoints: builder => ({
    tokenRefresh: builder.mutation<TokenResponse, void>({
      query: () => ({
        url: "/token/refresh",
        method: "POST",
      }),
    }),
  }),
  tagTypes: ["User", "Contact", "ChatRoom", "Message"],
})
