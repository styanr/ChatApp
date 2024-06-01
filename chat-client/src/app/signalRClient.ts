import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr"

import { setupSignalRListeners } from "./signalRHandlers"

let connection: HubConnection | null = null

const createConnection = (token: string) => {
  if (connection) {
    console.log("Returning existing connection.")
    return connection
  }

  console.log("Creating new connection. Token: ", token)

  connection = new HubConnectionBuilder()
    .withUrl("http://localhost:5117/chat", {
      accessTokenFactory: () => token,
    })
    .configureLogging(LogLevel.Information)
    .build()

  setupSignalRListeners(connection)

  return connection
}

export const getConnection = (): signalR.HubConnection | null => connection

export default createConnection
