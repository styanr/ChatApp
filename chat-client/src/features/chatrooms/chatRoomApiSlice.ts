import { apiSlice } from "../../app/api/apiSlice"

interface Message {
  id: string
  chatRoomId: string
  authorId: string
  content: string
  createdAt: string
  editedAt: string
  isDeleted: boolean
}

interface ChatRoomSummary {
  id: string
  name: string
  description: string
  pictureUrl: string
  createdAt: string
  lastMessage: Message
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
      providesTags: ["ChatRoom"],
    }),
    getChatRoomById: builder.query<ChatRoomSummary, string>({
      query: id => `chatrooms/${id}`,
      providesTags: (result, error, id) => [{ type: "ChatRoom", id }],
    }),
    createGroupChatRoom: builder.mutation<
      ChatRoomSummary,
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
      ChatRoomSummary,
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
      ChatRoomSummary,
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
