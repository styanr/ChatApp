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
}
