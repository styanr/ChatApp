import { FC, useState, useRef, useEffect } from "react"
import { useGetUserByIdQuery } from "../features/users/usersApiSlice"
import { convertUTCtoLocal } from "../utils/converters"

import { useLongPress } from "@uidotdev/usehooks"
import useFiles from "../app/hooks/useFiles"

import { ControlledMenu, MenuItem, MenuButton } from "@szhsin/react-menu"
import { RiDeleteBin6Fill, RiEdit2Fill, RiFile2Fill } from "react-icons/ri"
import ProfileImage from "./ProfileImage"
import { Link } from "react-router-dom"
interface AttachmentProps {
  attachmentId: string
}

const Attachment: FC<AttachmentProps> = ({ attachmentId }) => {
  const { getFile, isLoading } = useFiles()
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

  if (isLoading) {
    return (
      <div className="p-2 bg-ca-light-blue rounded-xl relative flex flex-col items-center justify-center">
        <RiFile2Fill className="w-10 h-10" />
      </div>
    )
  }

  return (
    <>
      <div className="flex items-center gap-2 pb-3">
        {file.type.startsWith("image/") && (
          <img
            src={file.url}
            alt="Attachment"
            className="h-auto rounded-md shadow-md cursor-pointer w-full object-cover max-h-96"
            onClick={toggleFullScreen}
          />
        )}
        {file.type.startsWith("video/") && (
          <video
            controls
            className="h-auto rounded-md shadow-md cursor-pointer w-full"
          >
            <source src={file.url} type={file.type} />
            Your browser does not support the video tag.
          </video>
        )}
        {file.type.startsWith("audio/") && (
          <audio controls={true} className="rounded-md shadow-md w-full">
            <source src={file.url} type={file.type} />
            Your browser does not support the audio tag.
          </audio>
        )}
        {!file.type.startsWith("image/") &&
          !file.type.startsWith("video/") &&
          !file.type.startsWith("audio/") && (
            <a href={file.url} download className="flex items-center gap-2">
              <div className="p-2 bg-ca-light-blue rounded-xl relative flex flex-col items-center justify-center">
                <RiFile2Fill className="w-10 h-10" />
                <div className="flex justify-center items-center text-white text-xs">
                  {file.type}
                </div>
              </div>
              <div>Download</div>
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

  const isNoContent = !message.content && message.attachmentId

  return (
    <div
      className={`flex gap-2 ${isCurrentUser ? "justify-end" : "justify-start"} mb-2`}
    >
      <div className="flex gap-2 items-end">
        {!isCurrentUser && (
          <Link to={`/contacts/${author?.id}`} className="flex items-center">
            <ProfileImage id={author?.profilePictureId} size={12} />
          </Link>
        )}
        <div>
          <div
            className={`flex flex-col ${
              isCurrentUser ? "items-start" : "items-end"
            } rounded-2xl w-fit max-w-80 md:max-w-2xl ${
              isNoContent
                ? ""
                : isCurrentUser
                  ? "px-5 py-3 bg-ca-blue"
                  : "px-5 py-3 bg-ca-gray"
            }`}
            ref={ref}
            {...attrs}
          >
            <ControlledMenu
              state={isMenuOpen ? "open" : "closed"}
              onClose={() => setIsMenuOpen(false)}
              anchorRef={ref}
              menuClassName="box-border z-50 text-sm bg-ca-dark py-3 border rounded-md shadow-lg select-none focus:outline-none min-w-[9rem] border-none w-48"
            >
              <MenuItem
                className="px-3 py-3 focus:bg-ca-dark-gray"
                onClick={() => setIsEditing(true)}
              >
                <div className="flex items-center flex-row">
                  <RiEdit2Fill className="w-6 h-6 pr-2" />
                  Edit
                </div>
              </MenuItem>
              <MenuItem
                className="px-3 py-3 focus:bg-ca-dark-gray"
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
                  className="text-white bg-ca-dark-blue rounded p-1"
                />
                <button onClick={handleEdit} className="text-ca-dark-blue ml-2">
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
                <div className={`text-xs`}>
                  {message.editedAt
                    ? `edited ${convertUTCtoLocal(message.editedAt)}`
                    : convertUTCtoLocal(message.createdAt)}
                </div>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}

export default Message
