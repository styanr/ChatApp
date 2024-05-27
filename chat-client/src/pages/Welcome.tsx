import React, { FC } from "react"

import { selectToken } from "../features/auth/authSlice"
import { useAppSelector } from "../app/store"

interface WelcomeProps {}

const Welcome: FC<WelcomeProps> = ({}) => {
  const token = useAppSelector(selectToken)
  return <div>Welcome. Token: {token?.slice(0, 9)}...</div>
}

export default Welcome
