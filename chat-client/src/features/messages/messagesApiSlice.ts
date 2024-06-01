import { apiSlice } from "../../app/api/apiSlice"
import { getConnection } from "../../app/signalRClient"
interface GetMessagesRequest {
  chatRoomId: string
  page: number
}

interface Message {
  id: string
  chatRoomId: string
  authorId: string
  content: string
  createdAt: string
  editedAt: string
  isDeleted: boolean
}

interface MessageCreate {
  chatRoomId: string
  content: string
}

export const messagesApiSlice = apiSlice.injectEndpoints({
  endpoints: builder => ({
    getMessages: builder.query<Message[], GetMessagesRequest>({
      query: ({ chatRoomId, page }) => `messages/${chatRoomId}?Page=${page}`,
      providesTags: (result, error, { chatRoomId }) => [{ type: "Message", id: chatRoomId }],
    }),
    sendMessage: builder.mutation<void, MessageCreate>({
      queryFn: async message => {
        const connection = getConnection()

        if (!connection) {
          throw new Error("SignalR connection not initialized")
        }

        return await connection.invoke("SendMessage", message.chatRoomId, {
          content: message.content,
        })
      },
    }),
  }),
})

export type { Message, GetMessagesRequest }
export const { useGetMessagesQuery, useSendMessageMutation } = messagesApiSlice
