import { FC, useState, useRef, useEffect } from "react"
import { useGetUserByIdQuery } from "../features/users/usersApiSlice"
import { convertUTCtoLocal } from "../utils/converters"

import { useLongPress } from "@uidotdev/usehooks"
import useFiles from "../app/hooks/useFiles"

import { ControlledMenu, MenuItem, MenuButton } from "@szhsin/react-menu"
import { RiDeleteBin6Fill, RiEdit2Fill } from "react-icons/ri"

interface AttachmentProps {
  attachmentId: string
}

const Attachment: FC<AttachmentProps> = ({ attachmentId }) => {
  const { getFile } = useFiles()
  const [file, setFile] = useState<{ url: string; type: string }>({
    url: "",
    type: "",
  })
  const [isFullScreen, setIsFullScreen] = useState(false)

  useEffect(() => {
    if (attachmentId) {
      getFile(attachmentId).then(file => {
        setFile({ url: URL.createObjectURL(file), type: file.type })
      })
    }
  }, [attachmentId])

  const toggleFullScreen = () => {
    setIsFullScreen(!isFullScreen)
  }

  return (
    <>
      <div className="flex items-center gap-2 pb-3">
        {file.type.startsWith("image/") && (
          <img
            src={file.url}
            alt="Attachment"
            className="h-auto rounded-md shadow-md cursor-pointer md:max-h-96 md:max-w-96 object-cover max-h-48 max-w-48"
            onClick={toggleFullScreen}
          />
        )}
        {file.type.startsWith("video/") && (
          <video
            controls
            className="h-auto rounded-md shadow-md cursor-pointer md:max-h-96 md:max-w-96 object-cover max-h-48 max-w-48"
          >
            <source src={file.url} type={file.type} />
            Your browser does not support the video tag.
          </video>
        )}
        {!file.type.startsWith("image/") && !file.type.startsWith("video/") && (
          <a href={file.url} download className="text-blue-500 underline">
            Download Attachment
          </a>
        )}
      </div>

      {isFullScreen && file.type.startsWith("image/") && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50"
          onClick={toggleFullScreen}
        >
          <img
            src={file.url}
            alt="Attachment"
            className="max-w-full max-h-full"
          />
        </div>
      )}
    </>
  )
}

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
                <>
                  {message.attachmentId && (
                    <Attachment attachmentId={message.attachmentId} />
                  )}
                  {message.content}
                </>
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
