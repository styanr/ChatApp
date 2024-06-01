import createConnection from "./signalRClient"
import { setupSignalRListeners } from "./signalRHandlers"
import { RootState, store } from "./store"
import { apiSlice } from "./api/apiSlice"
import { setCredentials, logOut } from "../features/auth/authSlice"

const handleTokenRefresh = async (): Promise<string | null> => {
  console.log("Refreshing token...")
  const refreshResult = await store.dispatch(
    apiSlice.endpoints.tokenRefresh.initiate(),
  )

  if (refreshResult?.data) {
    const refreshData = refreshResult.data as { accessToken: string }
    console.log("Token refreshed: ", refreshData.accessToken)
    const state = store.getState() as RootState

    store.dispatch(
      setCredentials({
        accessToken: refreshData.accessToken,
      }),
    )

    return refreshData.accessToken
  } else {
    store.dispatch(logOut())
    return null
  }
}

let connection: signalR.HubConnection | null = null

export const initializeConnection = async (): Promise<void> => {
  let state = store.getState() as RootState
  let token = state.auth.token

  // Wait for store rehydration
  while (!token) {
    await new Promise(resolve => setTimeout(resolve, 100))
    state = store.getState() as RootState
    token = state.auth.token as string
  }

  connection = createConnection(token)

  const startConnection = async () => {
    try {
      await connection?.start()
      console.log("Connection started")
    } catch (error) {
      console.error("Error starting connection: ", error)
    }
  }

  console.log("Connection state: ", connection?.state)
  if (connection.state === "Disconnected") {
    await startConnection()
  }

  connection.onclose(async () => {
    console.log("onclose: Connection closed")
    token = await handleTokenRefresh()
    if (token) {
      connection = createConnection(token)
      console.log("onclose: Reconnecting...")
      await startConnection()
    }
  })
}

initializeConnection()
