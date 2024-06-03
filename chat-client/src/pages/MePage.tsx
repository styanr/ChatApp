import React, { FC } from "react"

import { useAppDispatch } from "../app/store"
import { logOut } from "../features/auth/authSlice"
import { stopConnection } from "../app/signalRConnection"

interface MePageProps {}

const MePage: FC<MePageProps> = ({}) => {
  const dispatch = useAppDispatch()

  const handleLogOut = () => {
    dispatch(logOut())
    stopConnection()
  }

  return (
    <div className="bg-slate-950 flex-1 text-slate-100">
      <div className="">Me.</div>

      <button
        onClick={handleLogOut}
        className="bg-red-500 text-white p-2 rounded-md mt-4"
      >
        Log out
      </button>
    </div>
  )
}

export default MePage
