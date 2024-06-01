import React from "react"
import "./index.css"
import { createRoot } from "react-dom/client"

import { store } from "./app/store"
import { Provider } from "react-redux"

import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom"

import App from "./App"

import { initializeConnection } from "./app/signalRConnection"

import { PersistGate } from "redux-persist/integration/react"
import { persistStore } from "redux-persist"

const container = document.getElementById("root")

let persistor = persistStore(store)

initializeConnection()

if (container) {
  const root = createRoot(container)

  root.render(
    <React.StrictMode>
      <Provider store={store}>
        {/* <PersistGate loading={null} persistor={persistor}> */}
        <BrowserRouter>
          <App />
        </BrowserRouter>
        {/* </PersistGate> */}
      </Provider>
    </React.StrictMode>,
  )
} else {
  throw new Error(
    "Root element with ID 'root' was not found in the document. Ensure there is a corresponding HTML element with the ID 'root' in your HTML file.",
  )
}
