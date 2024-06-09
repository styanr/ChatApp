import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr"

import { setupSignalRListeners } from "./signalRHandlers"

let connection: HubConnection | null = null
let accessToken: string | null = null

const createConnection = (token: string) => {
  if (accessToken === token && connection) {
    console.log("Returning existing connection.")
    return connection
  }

  accessToken = token

  console.log("Creating new connection. Token: ", token)

  connection = new HubConnectionBuilder()
    .withUrl(import.meta.env.VITE_SERVER_URL + "chat", {
      accessTokenFactory: () => token,
    })
    .configureLogging(LogLevel.Information)
    .build()

  setupSignalRListeners(connection)

  return connection
}

export const getConnection = (): signalR.HubConnection | null => connection

export default createConnection
