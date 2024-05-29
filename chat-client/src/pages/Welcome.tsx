import React, { FC } from "react"
import { selectToken } from "../features/auth/authSlice"
import { useAppSelector } from "../app/store"

interface WelcomeProps {}

const Welcome: FC<WelcomeProps> = ({}) => {
  const token = useAppSelector(selectToken)
  return (
    <div className="bg-slate-950 flex-1">
      {/* Your Welcome component content here */}
    </div>
  )
}

export default Welcome
