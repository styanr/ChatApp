import { apiSlice } from "../../app/api/apiSlice"

import { Message } from "../messages/messagesApiSlice"

interface ChatRoomSummary {
  id: string
  name: string
  description: string
  pictureId: string
  createdAt: string
  type: "direct" | "group"
  lastMessage: Message
}

interface ChatRoomDetails {
  id: string
  name: string
  description: string
  pictureId: string
  createdAt: string
  type: "direct" | "group"
  userIds: string[]
}

interface GroupChatRoomCreateRequest {
  name: string
  pictureId: string
}

interface GroupChatRoomUpdateRequest {
  id: string
  name: string
  pictureId: string
  description: string
}

interface DirectChatRoomCreateRequest {
  otherUserId: string
}

interface GroupChatRoomAddUsersRequest {
  id: string
  userIds: string[]
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
      invalidatesTags: (result, error, { id }) => [
        { type: "ChatRoom", id },
        { type: "ChatRoom", id: "LIST" },
      ],
    }),
    addUsersToGroupChatRoom: builder.mutation<
      ChatRoomDetails,
      GroupChatRoomAddUsersRequest
    >({
      query: ({ userIds, ...body }) => ({
        url: `chatrooms/${body.id}/users`,
        method: "POST",
        body: JSON.stringify({ userIds }),
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
  useAddUsersToGroupChatRoomMutation,
} = chatRoomApiSlice

export type {
  Message,
  ChatRoomSummary,
  ChatRoomDetails,
  GroupChatRoomCreateRequest,
  GroupChatRoomUpdateRequest,
  DirectChatRoomCreateRequest,
  GroupChatRoomAddUsersRequest,
}
