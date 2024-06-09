import { apiSlice } from "../../app/api/apiSlice"

import { Message } from "../messages/messagesApiSlice"
import { getConnection } from "../../app/signalRClient"

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
    createGroupChatRoom: builder.mutation<void, GroupChatRoomCreateRequest>({
      queryFn: async body => {
        const connection = getConnection()

        if (!connection) {
          throw new Error("SignalR connection not initialized")
        }

        await connection.invoke("CreateGroupChatRoom", body)

        return { data: undefined }
      },
      invalidatesTags: ["ChatRoom"],
    }),
    updateGroupChatRoom: builder.mutation<void, GroupChatRoomUpdateRequest>({
      queryFn: async body => {
        const connection = getConnection()

        if (!connection) {
          throw new Error("SignalR connection not initialized")
        }

        await connection.invoke("UpdateGroupChatRoom", body.id, body)

        return { data: undefined }
      },
      invalidatesTags: (result, error, { id }) => [
        { type: "ChatRoom", id },
        { type: "ChatRoom", id: "LIST" },
      ],
    }),
    addUsersToGroupChatRoom: builder.mutation<
      void,
      GroupChatRoomAddUsersRequest
    >({
      queryFn: async body => {
        const connection = getConnection()

        if (!connection) {
          throw new Error("SignalR connection not initialized")
        }

        await connection.invoke("AddUsersToGroupChatRoom", body.id, body)

        return { data: undefined }
      },
      invalidatesTags: (result, error, { id }) => [{ type: "ChatRoom", id }],
    }),
    createDirectChatRoom: builder.mutation<
      ChatRoomDetails,
      DirectChatRoomCreateRequest
    >({
      queryFn: async body => {
        const connection = getConnection()

        if (!connection) {
          throw new Error("SignalR connection not initialized")
        }

        const chatRoom = await connection.invoke("CreateDirectChatRoom", body)

        return { data: chatRoom }
      },
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
