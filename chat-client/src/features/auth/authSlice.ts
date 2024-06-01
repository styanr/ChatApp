import { createSlice } from "@reduxjs/toolkit"
import { PayloadAction } from "@reduxjs/toolkit"

interface AuthState {
  token: string | null
}

const initialState: AuthState = {
  token: null,
}

interface Credentials {
  accessToken: string
}

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    setCredentials: (state, action: PayloadAction<Credentials>) => {
      const { accessToken } = action.payload
      state.token = accessToken
    },

    logOut: state => {
      state.token = null
    },
  },
  selectors: {
    selectToken: state => state.token,
  },
})

export const { setCredentials, logOut } = authSlice.actions
export const { selectToken } = authSlice.selectors

export default authSlice.reducer
