import { apiSlice } from "../../app/api/apiSlice"

interface ContactSearchResult {
  id: string
  handle: string | null
  displayName: string
  bio: string | null
  profilePictureUrl: string | undefined
  isContact: boolean
}

interface ContactsListResponse {
  users: ContactSearchResult[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

interface UsersQuery {
  searchTerm: string
  page: number
}

interface UserUpdate {
  handle: string | null
  displayName: string
  bio: string | null
  profilePictureUrl: string | undefined
}

export const usersApiSlice = apiSlice.injectEndpoints({
  endpoints: builder => ({
    getUsers: builder.query<ContactsListResponse, UsersQuery>({
      query: (contactsQuery: UsersQuery) =>
        `users?SearchTerm=${contactsQuery.searchTerm}&Page=${contactsQuery.page}`,
    }),

    getUserById: builder.query<ContactSearchResult, string>({
      query: id => `users/${id}`,
      providesTags: (result, error, arg) => [{ type: "User", id: arg }],
    }),

    getCurrentUser: builder.query<ContactSearchResult, void>({
      query: () => "users/current",
      providesTags: [{ type: "User", id: "current" }],
    }),

    updateCurrentUser: builder.mutation<ContactSearchResult, UserUpdate>({
      query: user => ({
        url: "users/current",
        method: "PUT",
        body: user,
      }),
      invalidatesTags: [{ type: "User", id: "current" }],
    }),
  }),
})

export const {
  useGetUsersQuery,
  useGetUserByIdQuery,
  useGetCurrentUserQuery,
  useUpdateCurrentUserMutation,
} = usersApiSlice

export type {
  ContactSearchResult,
  ContactsListResponse,
  UsersQuery as ContactsQuery,
  UserUpdate,
}
