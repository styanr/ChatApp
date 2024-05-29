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
  }),
})

export const { useGetUsersQuery, useGetUserByIdQuery } = usersApiSlice

export type {
  ContactSearchResult,
  ContactsListResponse,
  UsersQuery as ContactsQuery,
}
