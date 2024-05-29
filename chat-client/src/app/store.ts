import {
  configureStore,
  ThunkAction,
  Action,
  combineReducers,
} from "@reduxjs/toolkit"
import { useDispatch } from "react-redux"
import { apiSlice } from "./api/apiSlice"
import authReducer from "../features/auth/authSlice"
import { TypedUseSelectorHook, useSelector } from "react-redux"

import storage from "redux-persist/lib/storage"
import { persistReducer } from "redux-persist"

const combinedReducer = combineReducers({
  [apiSlice.reducerPath]: apiSlice.reducer,
  auth: authReducer,
})

const rootReducer = (state: any, action: any) => {
  console.log("rootReducer", action)
  if (action.type === "auth/logOut") {
    console.log("logOut")
    return combinedReducer(undefined, action)
  }
  return combinedReducer(state, action)
}

const persistConfig = {
  key: "root",
  storage,
}

const persistedReducer = persistReducer(persistConfig, rootReducer)

export const store = configureStore({
  reducer: persistedReducer,
  middleware: getDefaultMiddleware => {
    return getDefaultMiddleware().concat(apiSlice.middleware)
  },
  devTools: true,
})

export type AppDispatch = typeof store.dispatch
export const useAppDispatch = useDispatch.withTypes<AppDispatch>()
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector

export type RootState = ReturnType<typeof store.getState>
export type AppThunk<ReturnType = void> = ThunkAction<
  ReturnType,
  RootState,
  unknown,
  Action<string>
>
