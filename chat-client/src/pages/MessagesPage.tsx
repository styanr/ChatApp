import React, { FC, useState } from "react"
import { RiAddLargeFill } from "react-icons/ri"
import {
  useGetChatRoomsQuery,
  useCreateDirectChatRoomMutation,
  useCreateGroupChatRoomMutation,
  useAddUsersToGroupChatRoomMutation,
  useUpdateGroupChatRoomMutation,
} from "../features/chatrooms/chatRoomApiSlice"

import {
  GroupChatRoomCreateRequest,
  DirectChatRoomCreateRequest,
  GroupChatRoomAddUsersRequest,
} from "../features/chatrooms/chatRoomApiSlice"

import { useGetContactsQuery } from "../features/contacts/contactsApiSlice"

import { convertUTCtoLocal } from "../utils/converters"

import { Link } from "react-router-dom"
import ProfileImage from "../components/ProfileImage"

interface MessagesPageProps {}

interface CreateChatRoomModalProps {
  onSave: (
    createChatRoomRequest:
      | GroupChatRoomCreateRequest
      | DirectChatRoomCreateRequest,
  ) => void
  onCancel: () => void
  errors: any
}

const CreateChatRoomModal: FC<CreateChatRoomModalProps> = ({
  onSave,
  onCancel,
  errors,
}) => {
  const [newChatRoomType, setNewChatRoomType] = useState<"group" | "direct">(
    "group",
  )
  const [groupChatRoomRequest, setGroupChatRoomRequest] =
    useState<GroupChatRoomCreateRequest>({ name: "", pictureId: "" })
  const [directChatRoomRequest, setDirectChatRoomRequest] =
    useState<DirectChatRoomCreateRequest>({ otherUserId: "" })

  const { data: contacts, isLoading: isContactsLoading } = useGetContactsQuery()

  const handleInputChange = (
    e:
      | React.ChangeEvent<HTMLInputElement>
      | React.ChangeEvent<HTMLSelectElement>,
  ) => {
    if (newChatRoomType === "group") {
      setGroupChatRoomRequest({
        ...groupChatRoomRequest,
        [e.target.name]: e.target.value,
      })
    } else {
      console.log(e.target.value)
      setDirectChatRoomRequest({
        ...directChatRoomRequest,
        otherUserId: e.target.value,
      })
    }
  }

  const handleSave = () => {
    if (newChatRoomType === "group") {
      onSave(groupChatRoomRequest)
    } else {
      console.log(directChatRoomRequest)
      onSave(directChatRoomRequest)
    }
  }

  const IsEnabled = () => {
    if (newChatRoomType === "group") {
      console.log(groupChatRoomRequest.name.length)
      return groupChatRoomRequest.name.length > 0
    } else {
      console.log(directChatRoomRequest.otherUserId.length)
      return directChatRoomRequest.otherUserId.length > 0
    }
  }

  return (
    <div className="fixed inset-0 flex items-center justify-center z-50 bg-black bg-opacity-50">
      <div className="bg-slate-800 rounded-lg shadow-lg p-6">
        <h2 className="text-xl font-bold mb-4">Create Chat Room</h2>
        {errors.map(
          (error: any, index: number) =>
            error && (
              <div key={index} className="text-red-500 mb-4">
                {error?.data?.message || "An error occurred"}
              </div>
            ),
        )}
        <select
          className="rounded-md px-3 py-2 w-full mb-4 bg-gray-900 text-white"
          onChange={e => setNewChatRoomType(e.target.value as any)}
        >
          <option value="group">Group</option>
          <option value="direct">Direct</option>
        </select>
        {newChatRoomType === "group" && (
          <>
            <input
              type="text"
              name="name"
              className="rounded-md px-3 py-2 w-full mb-4 bg-gray-900 text-white"
              value={groupChatRoomRequest.name}
              onChange={handleInputChange}
              placeholder="Group Name"
            />
            <input
              type="text"
              name="pictureUrl"
              className="rounded-md px-3 py-2 w-full mb-4 bg-gray-900 text-white"
              value={groupChatRoomRequest.pictureId}
              onChange={handleInputChange}
              placeholder="Picture URL"
            />
          </>
        )}
        {newChatRoomType === "direct" && (
          <>
            {isContactsLoading ? (
              <div className="flex justify-center items-center h-full">
                <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-gray-500"></div>
              </div>
            ) : (
              <select
                className="rounded-md px-3 py-2 w-full mb-4 bg-gray-900 text-white"
                onChange={handleInputChange}
              >
                <option value="" disabled selected>
                  Select Contact
                </option>

                {contacts?.users.map(contact => (
                  <option key={contact.id} value={contact.id}>
                    {contact.displayName}
                  </option>
                ))}
              </select>
            )}
          </>
        )}
        <div className="flex justify-end">
          <button
            className="bg-gray-900 hover:bg-gray-800 text-white font-bold py-2 px-4 rounded mr-2"
            onClick={onCancel}
          >
            Cancel
          </button>
          <button
            className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded disabled:opacity-50 disabled:pointer-events-none"
            onClick={handleSave}
            disabled={!IsEnabled()}
          >
            Save
          </button>
        </div>
      </div>
    </div>
  )
}

const MessagesPage: FC<MessagesPageProps> = ({}) => {
  const { data, error, isLoading } = useGetChatRoomsQuery()
  const [showCreateChatRoomModal, setShowCreateChatRoomModal] = useState(false)
  const [
    createDirectChatRoom,
    { isLoading: isCreatingDirectChatRoom, error: directChatRoomError },
  ] = useCreateDirectChatRoomMutation()
  const [
    createGroupChatRoom,
    { isLoading: isCreatingGroupChatRoom, error: groupChatRoomError },
  ] = useCreateGroupChatRoomMutation()

  const handleCreateChatRoom = async (
    createChatRoomRequest:
      | GroupChatRoomCreateRequest
      | DirectChatRoomCreateRequest,
  ) => {
    try {
      if ("name" in createChatRoomRequest) {
        await createGroupChatRoom(createChatRoomRequest).unwrap()
      } else {
        await createDirectChatRoom(createChatRoomRequest).unwrap()
      }
      setShowCreateChatRoomModal(false)
    } catch (error) {
      console.error("Error creating chat room:", error)
    }
  }

  return (
    <div className="flex-1 bg-slate-900 text-white flex overflow-y-auto mb-20 relative">
      {showCreateChatRoomModal && (
        <CreateChatRoomModal
          onSave={handleCreateChatRoom}
          onCancel={() => setShowCreateChatRoomModal(false)}
          errors={[directChatRoomError, groupChatRoomError]}
        />
      )}

      {isLoading && (
        <div className="flex justify-center items-center h-full">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-gray-500"></div>
        </div>
      )}
      {error && (
        <div className="flex justify-center items-center h-full text-red-500">
          Error
        </div>
      )}
      {data && (
        <div className="h-full w-full relative">
          {data.length === 0 ? (
            <div className="flex p-10 h-full size-20 text-xl w-full font-bold">
              No chat rooms :(
            </div>
          ) : (
            <div className="flex flex-col w-full">
              {[...data]
                .sort((a, b) => {
                  if (
                    (a.lastMessage?.createdAt ?? a.createdAt) >
                    (b.lastMessage?.createdAt ?? b.createdAt)
                  ) {
                    return -1
                  }
                  return 1
                })
                .map(
                  (chatRoom, index) => (
                    console.log(chatRoom),
                    (
                      <Link
                        to={`/messages/${chatRoom.id}`}
                        key={chatRoom.id}
                        className={`flex items-center px-5 py-4 bg-${index % 2 === 0 ? "slate-900" : "gray-800"} hover:bg-indigo-900 active:bg-indigo-900 transition-colors`}
                      >
                        <ProfileImage id={chatRoom.pictureId} size={12} />
                        <div className="flex flex-row w-full justify-between items-center">
                          <div className="flex flex-col ml-5">
                            <span className="text-lg font-semibold">
                              {chatRoom.name}
                            </span>
                            <span className="text-sm">
                              {chatRoom.lastMessage
                                ? chatRoom.lastMessage.content
                                : "No messages"}
                            </span>
                          </div>
                          <div className="flex flex-col ml-3">
                            <span className="text-sm">
                              {convertUTCtoLocal(
                                chatRoom.lastMessage
                                  ? chatRoom.lastMessage.createdAt
                                  : chatRoom.createdAt,
                              )}
                            </span>
                          </div>
                        </div>
                      </Link>
                    )
                  ),
                )}
            </div>
          )}
          <div className="absolute bottom-0 right-0 p-5">
            <button
              className="bg-blue-500 p-3 rounded-xl hover:bg-blue-400"
              onClick={() => setShowCreateChatRoomModal(true)}
              disabled={isCreatingDirectChatRoom || isCreatingGroupChatRoom}
            >
              <RiAddLargeFill size={24} />
            </button>
          </div>
        </div>
      )}
    </div>
  )
}

export default MessagesPage
