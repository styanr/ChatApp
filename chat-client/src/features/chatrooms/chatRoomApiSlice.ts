import { apiSlice } from "../../app/api/apiSlice"

import { Message } from "../messages/messagesApiSlice"

interface ChatRoomSummary {
  id: string
  name: string
  description: string
  pictureUrl: string
  createdAt: string
  type: "direct" | "group"
  lastMessage: Message
}

interface ChatRoomDetails {
  d: string
  name: string
  description: string
  pictureUrl: string
  createdAt: string
  type: "direct" | "group"
  userIds: string[]
}

interface GroupChatRoomCreateRequest {
  name: string
  pictureUrl: string
}

interface GroupChatRoomUpdateRequest {
  id: string
  name: string
  pictureUrl: string
  description: string
}

interface DirectChatRoomCreateRequest {
  otherUserId: string
}

export const chatRoomApiSlice = apiSlice.injectEndpoints({
  endpoints: builder => ({
    getChatRooms: builder.query<ChatRoomSummary[], void>({
      query: () => "chatrooms",
      providesTags: [{ type: "ChatRoom", id: "LIST" }],
    }),
    getChatRoomById: builder.query<ChatRoomDetails, string>({
      query: id => `chatrooms/${id}`,
      providesTags: (result, error, id) => [{ type: "ChatRoom", id }],
    }),
    createGroupChatRoom: builder.mutation<
      ChatRoomDetails,
      GroupChatRoomCreateRequest
    >({
      query: body => ({
        url: "chatrooms/group",
        method: "POST",
        body: JSON.stringify(body),
      }),
      invalidatesTags: ["ChatRoom"],
    }),
    updateGroupChatRoom: builder.mutation<
      ChatRoomDetails,
      GroupChatRoomUpdateRequest
    >({
      query: ({ id, ...body }) => ({
        url: `chatrooms/${id}`,
        method: "PUT",
        body: JSON.stringify(body),
      }),
      invalidatesTags: (result, error, { id }) => [{ type: "ChatRoom", id }],
    }),
    createDirectChatRoom: builder.mutation<
      ChatRoomDetails,
      DirectChatRoomCreateRequest
    >({
      query: body => ({
        url: "chatrooms/direct",
        method: "POST",
        body: JSON.stringify(body),
      }),
      invalidatesTags: ["ChatRoom"],
    }),
  }),
})

export const {
  useGetChatRoomsQuery,
  useGetChatRoomByIdQuery,
  useCreateGroupChatRoomMutation,
  useUpdateGroupChatRoomMutation,
  useCreateDirectChatRoomMutation,
} = chatRoomApiSlice

export type {
  Message,
  ChatRoomSummary,
  GroupChatRoomCreateRequest,
  GroupChatRoomUpdateRequest,
  DirectChatRoomCreateRequest,
}
