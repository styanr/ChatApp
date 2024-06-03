import React, { FC, useState } from "react"

import { useAppDispatch } from "../app/store"
import { logOut } from "../features/auth/authSlice"
import { stopConnection } from "../app/signalRConnection"

import {
  UserUpdate,
  useGetCurrentUserQuery,
  useUpdateCurrentUserMutation,
} from "../features/users/usersApiSlice"
import ProfileImage from "../components/ProfileImage"
import ContactDisplay from "../components/ContactDisplay"
import EditProfileModal from "../components/EditProfileModal"

interface MePageProps {}

const MePage: FC<MePageProps> = ({}) => {
  const dispatch = useAppDispatch()

  const handleLogOut = () => {
    dispatch(logOut())
    stopConnection()
  }

  const { data: user, error, isLoading } = useGetCurrentUserQuery()
  const [updateUser] = useUpdateCurrentUserMutation()

  const [showEditModal, setShowEditModal] = useState(false)

  const onSave = (user: UserUpdate) => {
    updateUser(user)
    setShowEditModal(false)
  }

  const onCancel = () => {
    setShowEditModal(false)
  }

  if (isLoading)
    return (
      <div className="flex justify-center items-center h-full bg-slate-800">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-gray-500"></div>
      </div>
    )
  if (error) {
    if ("message" in error) {
      ;<div className="flex justify-center items-center h-full bg-slate-800">
        <p>Error: {error.message}</p>
      </div>
    }
  }

  return (
    <>
      {showEditModal && (
        <EditProfileModal user={user!} onSave={onSave} onCancel={onCancel} />
      )}
      <ContactDisplay
        data={user}
        isCurrentUser={true}
        onLogout={handleLogOut}
        onEdit={() => setShowEditModal(true)}
      />
    </>
  )
}

export default MePage
