import { apiSlice } from "../../app/api/apiSlice"

import {
  ContactsListResponse,
  ContactSearchResult,
} from "../users/usersApiSlice"

interface UpdateContactRequest {
  contactUserId: string
  displayName: string
}

export const contactsApiSlice = apiSlice.injectEndpoints({
  endpoints: builder => ({
    getContacts: builder.query<ContactsListResponse, void>({
      query: () => "contacts",
      providesTags: ["Contact"],
    }),
    addContact: builder.mutation<ContactSearchResult, string>({
      query: contactUserId => ({
        url: "contacts/",
        method: "POST",
        body: JSON.stringify({ contactUserId }),
      }),
      invalidatesTags: (result, error, contactUserId) => [
        { type: "Contact" },
        { type: "User", id: contactUserId },
      ],
    }),
    updateContact: builder.mutation<ContactSearchResult, UpdateContactRequest>({
      query: ({ contactUserId, displayName }) => ({
        url: `contacts/${contactUserId}`,
        method: "PUT",
        body: JSON.stringify({ displayName }),
      }),
      invalidatesTags: (result, error, { contactUserId }) => [
        { type: "Contact" },
        { type: "User", id: contactUserId },
        { type: "ChatRoom", id: "LIST" },
      ],
    }),
    deleteContact: builder.mutation<void, string>({
      query: contactUserId => ({
        url: `contacts/${contactUserId}`,
        method: "DELETE",
      }),
      invalidatesTags: (result, error, contactUserId) => [
        { type: "Contact" },
        { type: "User", id: contactUserId },
        { type: "ChatRoom", id: "LIST" },
      ],
    }),
  }),
})

export const {
  useGetContactsQuery,
  useAddContactMutation,
  useUpdateContactMutation,
  useDeleteContactMutation,
} = contactsApiSlice
