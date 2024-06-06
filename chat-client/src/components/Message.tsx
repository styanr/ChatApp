import { FC, useState, useRef } from "react"
import { useGetUserByIdQuery } from "../features/users/usersApiSlice"
import { convertUTCtoLocal } from "../utils/converters"

import { useLongPress } from "@uidotdev/usehooks"
import { ControlledMenu, MenuItem, MenuButton } from "@szhsin/react-menu"
import { RiDeleteBin6Fill, RiEdit2Fill } from "react-icons/ri"

interface MessageProps {
  message: any
  isCurrentUser: boolean
  isGroupChat: boolean
  onEditMessage: (id: string, newContent: string) => void
  onDeleteMessage: (id: string) => void
}

const Message: FC<MessageProps> = ({
  message,
  isCurrentUser,
  isGroupChat,
  onEditMessage,
  onDeleteMessage,
}) => {
  const ref = useRef(null)
  const attrs = useLongPress(() => {
    if (isCurrentUser && !message.isDeleted) {
      setIsMenuOpen(true)
    }
  })

  const {
    data: author,
    error: authorError,
    isLoading: authorLoading,
  } = useGetUserByIdQuery(message.authorId)

  const [isEditing, setIsEditing] = useState(false)
  const [newContent, setNewContent] = useState(message.content)
  const [isMenuOpen, setIsMenuOpen] = useState(false)

  const handleEdit = () => {
    onEditMessage(message.id, newContent)
    setIsEditing(false)
  }

  const handleDelete = () => {
    onDeleteMessage(message.id)
  }

  if (authorLoading) {
    return <div>Loading...</div>
  }

  if (authorError) {
    return <div>Error...</div>
  }

  return (
    <div
      className={`flex flex-col gap-2 ${isCurrentUser ? "items-end" : "items-start"}`}
    >
      {isGroupChat && author && (
        <div className="text-xs text-gray-400">{author.displayName}</div>
      )}
      <div
        className={`px-5 py-3 mb-2 rounded-2xl w-fit ${
          isCurrentUser
            ? "ml-auto bg-blue-500 text-right"
            : "mr-auto bg-slate-800 text-left"
        }`}
        ref={ref}
        {...attrs}
      >
        <ControlledMenu
          state={isMenuOpen ? "open" : "closed"}
          onClose={() => setIsMenuOpen(false)}
          anchorRef={ref}
          menuClassName="box-border z-50 text-sm bg-gray-800 py-3 border rounded-md shadow-lg select-none focus:outline-none min-w-[9rem] border-none w-48"
        >
          <MenuItem
            className="px-3 py-3 focus:bg-gray-700"
            onClick={() => setIsEditing(true)}
          >
            <div className="flex items-center flex-row">
              <RiEdit2Fill className="w-6 h-6 pr-2" />
              Edit
            </div>
          </MenuItem>
          <MenuItem
            className="px-3 py-3 focus:bg-gray-700"
            onClick={handleDelete}
          >
            <div className="flex items-center flex-row">
              <RiDeleteBin6Fill className="w-6 h-6 pr-2" />
              Delete
            </div>
          </MenuItem>
        </ControlledMenu>
        {isEditing ? (
          <>
            <input
              type="text"
              value={newContent}
              onChange={e => setNewContent(e.target.value)}
              className="text-white bg-slate-600 rounded p-1"
            />
            <button onClick={handleEdit} className="text-blue-400 ml-2">
              Save
            </button>
            <button
              onClick={() => setIsEditing(false)}
              className="text-red-400 ml-2"
            >
              Cancel
            </button>
          </>
        ) : (
          <>
            <div className="text-white">
              {message.isDeleted ? (
                <>
                  <span className="italic">This message was deleted</span>
                </>
              ) : (
                message.content
              )}
            </div>
            {message.editedAt ? (
              <div className="text-xs text-gray-400">
                edited {convertUTCtoLocal(message.editedAt)}
              </div>
            ) : (
              <div className="text-xs text-gray-400">
                {convertUTCtoLocal(message.createdAt)}
              </div>
            )}
          </>
        )}
      </div>
    </div>
  )
}

export default Message
