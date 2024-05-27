import { createSlice } from "@reduxjs/toolkit"
import { PayloadAction } from "@reduxjs/toolkit"

interface AuthState {
  user: any
  token: string | null
}

const initialState: AuthState = {
  user: null,
  token: null,
}

interface Credentials {
  user: any
  accessToken: string
}

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    setCredentials: (state, action: PayloadAction<Credentials>) => {
      const { user, accessToken } = action.payload
      state.user = user
      state.token = accessToken
    },

    logOut: state => {
      state.user = null
      state.token = null
    },
  },
  selectors: {
    selectUser: state => state.user,
    selectToken: state => state.token,
  },
})

export const { setCredentials, logOut } = authSlice.actions
export const { selectUser, selectToken } = authSlice.selectors

export default authSlice.reducer
