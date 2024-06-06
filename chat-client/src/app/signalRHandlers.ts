// signalRHandlers.ts
import { store } from "../app/store"
import { messagesApiSlice } from "../features/messages/messagesApiSlice"
import { apiSlice } from "../app/api/apiSlice"
import { Message } from "../features/messages/messagesApiSlice"

export const setupSignalRListeners = (connection: signalR.HubConnection) => {
  connection.on("ReceiveMessage", (message: Message) => {
    console.log("ReceiveMessage", message)
    store.dispatch(
      messagesApiSlice.util.updateQueryData(
        "getMessages",
        { chatRoomId: message.chatRoomId, page: 1 },
        draft => {
          draft.push(message)
        },
      ),
    )
    store.dispatch(
      apiSlice.util.invalidateTags([{ type: "ChatRoom", id: "LIST" }]),
    )
  })
  connection.on("ReceiveEditMessage", (message: Message) => {
    console.log("ReceiveEditMessage", message)
    store.dispatch(
      messagesApiSlice.util.updateQueryData(
        "getMessages",
        { chatRoomId: message.chatRoomId, page: 1 },
        draft => {
          const index = draft.findIndex(m => m.id === message.id)
          if (index !== -1) {
            draft[index] = message
          }
        },
      ),
    )
  })
  connection.on("ReceiveDeleteMessage", (chatRoomId: string, messageId: string) => {
    console.log("ReceiveDeleteMessage", messageId)
    store.dispatch(
      messagesApiSlice.util.updateQueryData(
        "getMessages",
        { chatRoomId: chatRoomId, page: 1 },
        draft => {
          const index = draft.findIndex(m => m.id === messageId)
          if (index !== -1) {
            draft[index].isDeleted = true
          }
        },
      ),
    )
  })
}
