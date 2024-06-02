import React, { FC, useState } from "react"

import {
  useUpdateGroupChatRoomMutation,
  useAddUsersToGroupChatRoomMutation,
  GroupChatRoomUpdateRequest,
  GroupChatRoomAddUsersRequest,
} from "../features/chatrooms/chatRoomApiSlice"

import { useGetUserByIdQuery } from "../features/users/usersApiSlice"

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
            <div className="flex justify-between items-center">
              <div className="flex items-center gap-4">
                <ProfileImage src={chatRoom.pictureUrl} size={20} />
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
              <h3 className="text-lg font-semibold mt-4">Members</h3>
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
    pictureUrl: chatRoom.pictureUrl,
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
          value={groupChat.pictureUrl}
          onChange={e =>
            setGroupChat({ ...groupChat, pictureUrl: e.target.value })
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
      <ProfileImage src={user.profilePictureUrl} size={10} />
      <p>{user.displayName}</p>
    </Link>
  )
}

export default ViewGroupChatModal
