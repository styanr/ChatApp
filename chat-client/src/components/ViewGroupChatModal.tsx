import React, { FC, useState } from "react"

import {
  useUpdateGroupChatRoomMutation,
  useAddUsersToGroupChatRoomMutation,
  GroupChatRoomUpdateRequest,
  GroupChatRoomAddUsersRequest,
} from "../features/chatrooms/chatRoomApiSlice"

import { useGetUserByIdQuery } from "../features/users/usersApiSlice"

import { useGetContactsQuery } from "../features/contacts/contactsApiSlice"

import ProfileImage from "../components/ProfileImage"

import { ChatRoomDetails } from "../features/chatrooms/chatRoomApiSlice"
import { Link } from "react-router-dom"

import { RiEdit2Fill, RiPencilFill } from "react-icons/ri"

interface ViewGroupChatModalProps {
  chatRoom: ChatRoomDetails
  onClose: () => void
}

const ViewGroupChatModal: FC<ViewGroupChatModalProps> = ({
  chatRoom,
  onClose,
}) => {
  const [isEditing, setIsEditing] = useState(false)
  const [isAddingUsers, setIsAddingUsers] = useState(false)

  return (
    <div className="fixed inset-0 bg-slate-800 shadow-lg p-6 w-full z-20 flex items-center justify-center">
      <div className="max-w-md w-full h-full">
        {isEditing ? (
          <EditGroupChatModal
            chatRoom={chatRoom}
            onClose={() => setIsEditing(false)}
          />
        ) : (
          <>
            {isAddingUsers && (
              <AddUsersToGroupChatModal
                chatRoom={chatRoom}
                onClose={() => setIsAddingUsers(false)}
              />
            )}
            <div className="flex justify-between items-center">
              <div className="flex items-center gap-4">
                <ProfileImage id={chatRoom.pictureId} size={20} />
                <div>
                  <h2 className="text-xl font-semibold">{chatRoom.name}</h2>
                  <p>
                    {chatRoom.userIds.length} member
                    {chatRoom.userIds.length > 1 && "s"}
                  </p>
                </div>
              </div>
              <div>
                <button
                  onClick={() => setIsEditing(true)}
                  className="text-white hover:bg-slate-700 rounded-lg transition-colors duration-300 p-3"
                >
                  <RiEdit2Fill className="w-5 h-5" />
                </button>
              </div>
            </div>
            <p className="text-white mt-5">
              {chatRoom.description || "No description"}
            </p>
            <div>
              <div className="flex justify-between items-center mt-4">
                <h3 className="text-lg font-semibold">Members</h3>
                <button
                  onClick={() => setIsAddingUsers(true)}
                  className="text-white hover:bg-slate-700 rounded-lg transition-colors duration-300 p-3"
                >
                  <RiPencilFill className="w-5 h-5" />
                </button>
              </div>
              <div className="flex flex-col mt-2 gap-2">
                {chatRoom.userIds.map(userId => (
                  <UserListItem key={userId} userId={userId} />
                ))}
              </div>
            </div>
            <div className="flex justify-end mt-4">
              <button
                onClick={onClose}
                className="bg-red-500 text-white rounded-lg py-2 px-3 mt-4 flex justify-center items-center hover:bg-red-600 transition-colors duration-300"
              >
                Close
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  )
}

const EditGroupChatModal: FC<ViewGroupChatModalProps> = ({
  chatRoom,
  onClose,
}) => {
  const [updateGroupChatRoom, { isLoading }] = useUpdateGroupChatRoomMutation()

  const [groupChat, setGroupChat] = useState<GroupChatRoomUpdateRequest>({
    id: chatRoom.id,
    name: chatRoom.name,
    pictureId: chatRoom.pictureId,
    description: chatRoom.description || "",
  })

  const onSave = () => {
    updateGroupChatRoom(groupChat).unwrap()
    onClose()
  }

  return (
    <div>
      <div>
        <label htmlFor="name" className="block text-white">
          Name
        </label>
        <input
          type="text"
          id="name"
          value={groupChat.name}
          onChange={e => setGroupChat({ ...groupChat, name: e.target.value })}
          className="bg-slate-700 text-white w-full p-2 rounded-lg mt-1"
        />

        <label htmlFor="description" className="block text-white mt-4">
          Description
        </label>
        <textarea
          id="description"
          value={groupChat.description}
          onChange={e =>
            setGroupChat({ ...groupChat, description: e.target.value })
          }
          className="bg-slate-700 text-white w-full p-2 rounded-lg mt-1"
        />

        <label htmlFor="picture" className="block text-white mt-4">
          Picture URL
        </label>
        <input
          type="text"
          id="picture"
          value={groupChat.pictureId}
          onChange={e =>
            setGroupChat({ ...groupChat, pictureId: e.target.value })
          }
          className="bg-slate-700 text-white w-full p-2 rounded-lg mt-1"
        />
      </div>

      <div className="flex justify-end mt-4">
        <button
          onClick={onClose}
          className="bg-red-500 text-white rounded-lg py-2 px-3 mt-4 flex justify-center items-center hover:bg-red-600 transition-colors duration-300"
        >
          Cancel
        </button>
        <button
          onClick={onSave}
          disabled={isLoading}
          className="bg-green-500 text-white rounded-lg py-2 px-3 mt-4 flex justify-center items-center hover:bg-green-600 transition-colors duration-300 ml-2"
        >
          {isLoading ? "Saving..." : "Save"}
        </button>
      </div>
    </div>
  )
}

interface ViewGroupChatModalProps {
  chatRoom: ChatRoomDetails
  onClose: () => void
}

const AddUsersToGroupChatModal: FC<ViewGroupChatModalProps> = ({
  chatRoom,
  onClose,
}) => {
  const [addUsersToGroupChatRoom, { isLoading }] =
    useAddUsersToGroupChatRoomMutation()
  const [userIds, setUserIds] = useState<string[]>([])
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  const {
    data: contacts,
    isLoading: isContactsLoading,
    error: contactsError,
  } = useGetContactsQuery()

  const onSave = async () => {
    try {
      await addUsersToGroupChatRoom({ id: chatRoom.id, userIds }).unwrap()
      onClose()
    } catch (error) {
      setErrorMessage((error as any).message)
    }
  }

  if (isContactsLoading || !contacts) {
    return <p>Loading...</p>
  }

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 z-30">
      <div className="w-full h-full flex flex-col items-center justify-center">
        <div className="bg-slate-800 p-6 rounded-lg">
          <h2 className="text-white text-lg font-semibold">Add Users</h2>

          <div className="flex flex-col gap-2 mt-4">
            {contacts.users.map(contact => (
              <label
                key={contact.id}
                className={`flex items-center justify-between gap-2 py-3 px-5 ${userIds.includes(contact.id) ? "bg-slate-700" : ""} rounded-lg transition-colors duration-300 min-w-80`}
              >
                <div className="flex items-center gap-2">
                  <ProfileImage id={contact.profilePictureId} size={10} />
                  <p className="ml-2">{contact.displayName}</p>
                </div>

                <input
                  type="checkbox"
                  checked={userIds.includes(contact.id)}
                  onChange={e => {
                    if (e.target.checked) {
                      setUserIds([...userIds, contact.id])
                    } else {
                      setUserIds(userIds.filter(id => id !== contact.id))
                    }
                  }}
                />
              </label>
            ))}
          </div>

          {errorMessage && (
            <p className="text-red-500 mt-2">
              Failed to add users: {errorMessage}
            </p>
          )}

          <div className="flex justify-end mt-4">
            <button
              onClick={onClose}
              className="bg-red-500 text-white rounded-lg py-2 px-3 mt-4 flex justify-center items-center hover:bg-red-600 transition-colors duration-300"
            >
              Cancel
            </button>
            <button
              onClick={onSave}
              disabled={isLoading}
              className="bg-green-500 text-white rounded-lg py-2 px-3 mt-4 flex justify-center items-center hover:bg-green-600 transition-colors duration-300 ml-2"
            >
              {isLoading ? "Adding..." : "Add"}
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}

const UserListItem: FC<{ userId: string }> = ({ userId }) => {
  const { data: user } = useGetUserByIdQuery(userId)

  if (!user) {
    return null
  }

  return (
    <Link
      className="flex items-center gap-4
    hover:bg-slate-700 px-2 py-2 rounded-lg transition-colors duration-300
    "
      to={`/contacts/${userId}`}
    >
      <ProfileImage id={user.profilePictureId} size={10} />
      <p>{user.displayName}</p>
    </Link>
  )
}

export default ViewGroupChatModal
