import { createSlice } from "@reduxjs/toolkit"
import { PayloadAction } from "@reduxjs/toolkit"

interface Contact {
  id: string
  handle: string | null
  displayName: string
  bio: string | null
  profilePictureUrl: string | undefined
  isContact: boolean
}


