import React from "react"
import { Outlet } from "react-router-dom"
import { useAppSelector } from "../app/store"
import { selectToken } from "../features/auth/authSlice"
import Navbar from "./Navbar"

const Layout: React.FC = () => {
  const token = useAppSelector(selectToken)
  return (
    <div className="min-h-screen">
      <div className="flex flex-col h-screen">
        <Outlet />
        {token && <Navbar />}
      </div>
    </div>
  )
}

export default Layout
