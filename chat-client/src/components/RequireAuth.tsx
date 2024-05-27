import { useLocation, Navigate, Outlet } from "react-router-dom"
import { useAppSelector } from "../app/store"
import { selectToken } from "../features/auth/authSlice"
import { FC } from "react"

const RequireAuth: FC = ({}) => {
  const token = useAppSelector(selectToken)
  const location = useLocation()

  console.log("RequireAuth", token, location)

  return token ? (
    <Outlet />
  ) : (
    <Navigate to="/login" state={{ from: location }} replace />
  )
}

export default RequireAuth
