import React, { FC, useState, useEffect, useRef } from "react"
import { Link, useParams } from "react-router-dom"
import {
  useGetMessagesQuery,
  useSendMessageMutation,
  useEditMessageMutation,
  useDeleteMessageMutation,
} from "../features/messages/messagesApiSlice"
import { useGetChatRoomByIdQuery } from "../features/chatrooms/chatRoomApiSlice"
import {
  useGetCurrentUserQuery,
  useGetUserByIdQuery,
} from "../features/users/usersApiSlice"
import { useGetContactsQuery } from "../features/contacts/contactsApiSlice"

import ProfileImage from "../components/ProfileImage"
import { convertUTCtoLocal } from "../utils/converters"

import ViewGroupChatRoomModal from "../components/ViewGroupChatModal"

import Message from "../components/Message"

interface ConversationPageProps {}

const ConversationPage: FC<ConversationPageProps> = () => {
  const { id } = useParams()
  const [page, setPage] = useState(1)
  const [messageContent, setMessageContent] = useState("")
  const [showScrollToBottom, setShowScrollToBottom] = useState(false)
  const [showInfoModal, setShowInfoModal] = useState(false)

  const {
    data: messages,
    error: messagesError,
    isLoading: messagesLoading,
  } = useGetMessagesQuery({
    chatRoomId: id as string,
    page,
  })

  const [sendMessage] = useSendMessageMutation()
  const [editMessage] = useEditMessageMutation()
  const [deleteMessage] = useDeleteMessageMutation()

  const {
    data: chatRoom,
    error: chatRoomError,
    isLoading: chatRoomLoading,
  } = useGetChatRoomByIdQuery(id as string)

  const {
    data: currentUser,
    error: currentUserError,
    isLoading: currentUserLoading,
  } = useGetCurrentUserQuery()

  const otherUserId = chatRoom?.userIds?.find(
    userId => userId !== currentUser?.id,
  )
  const {
    data: otherUser,
    error: otherUserError,
    isLoading: otherUserLoading,
  } = useGetUserByIdQuery(otherUserId!, { skip: !otherUserId })

  const messageEndRef = useRef<HTMLDivElement>(null)
  const messagesContainerRef = useRef<HTMLDivElement>(null)

  const scrollToBottom = () => {
    if (messageEndRef.current) {
      messageEndRef.current.scrollIntoView({ behavior: "smooth" })
    }
    setShowScrollToBottom(false)
  }

  useEffect(() => {
    console.log(messages)
    if (messagesContainerRef.current) {
      const { scrollTop, scrollHeight, clientHeight } =
        messagesContainerRef.current
      const isAtBottom = Math.abs(scrollTop - scrollHeight + clientHeight) < 100
      if (isAtBottom) {
        scrollToBottom()
      } else {
        setShowScrollToBottom(true)
      }
    }
  }, [messages])

  useEffect(() => {
    if (messagesContainerRef.current) {
      const { scrollHeight } = messagesContainerRef.current
      messagesContainerRef.current.scrollTop = scrollHeight
    }
  }, [id])

  const handleSendMessage = async () => {
    if (messageContent.trim()) {
      await sendMessage({ chatRoomId: id as string, content: messageContent })
      setMessageContent("")
      scrollToBottom()
    }
  }

  const handleEditMessage = async (messageId: string, newContent: string) => {
    await editMessage({ id: messageId, content: newContent })
  }

  const handleDeleteMessage = async (messageId: string) => {
    await deleteMessage(messageId)
  }

  if (
    messagesLoading ||
    chatRoomLoading ||
    currentUserLoading ||
    otherUserLoading
  ) {
    return (
      <div className="flex-1 bg-slate-900 text-white flex justify-center items-center">
        Loading...
      </div>
    )
  }

  if (messagesError || chatRoomError || currentUserError || otherUserError) {
    return (
      <div className="flex-1 bg-slate-900 text-white flex justify-center items-center">
        Error...
      </div>
    )
  }

  const isGroupChat = chatRoom?.type === "group"

  return (
    <div className="flex-1 mb-20 bg-slate-900 text-white flex flex-col overflow-hidden relative">
      {chatRoom && (
        <>
          {isGroupChat ? (
            <button
              className="fixed top-0 left-0 right-0 flex items-center px-5 py-4 bg-slate-800 hover:bg-gray-700 transition-colors duration-300 z-10"
              onClick={() => setShowInfoModal(true)}
            >
              <ProfileImage id={chatRoom.pictureId} size={12} />
              <h2 className="font-semibold ml-4">{chatRoom.name}</h2>
            </button>
          ) : (
            otherUser && (
              <Link
                to={`/contacts/${otherUserId}`}
                className="fixed top-0 left-0 right-0 flex items-center px-5 py-4 bg-slate-800 hover:bg-gray-700 transition-colors duration-300 z-10"
              >
                <ProfileImage id={otherUser.profilePictureId} size={12} />
                <h2 className="font-semibold ml-4">{otherUser.displayName}</h2>
              </Link>
            )
          )}
        </>
      )}
      <div
        className="flex-1 p-4 mt-20 overflow-y-auto"
        ref={messagesContainerRef}
      >
        {messages &&
          messages.map(message => (
            <Message
              key={message.id}
              message={message}
              isCurrentUser={message.authorId === currentUser?.id}
              isGroupChat={isGroupChat}
              onEditMessage={handleEditMessage}
              onDeleteMessage={handleDeleteMessage}
            />
          ))}
        <div ref={messageEndRef}></div>
      </div>
      {showScrollToBottom && (
        <button
          onClick={scrollToBottom}
          className="absolute bottom-24 right-4 bg-indigo-800 hover:bg-indigo-900 text-white font-bold py-2 px-4 rounded-full shadow-lg transition-colors duration-300 z-20 aspect-square"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-6 w-6"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M19 14l-7 7m0 0l-7-7m7 7V3"
            />
          </svg>
        </button>
      )}
      <div className="flex p-4 gap-2 bg-slate-800 z-10">
        <input
          type="text"
          className="w-full p-4 bg-slate-700 rounded text-white"
          placeholder="Type a message..."
          value={messageContent}
          onChange={e => setMessageContent(e.target.value)}
        />
        <button
          onClick={handleSendMessage}
          className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
        >
          Send
        </button>
      </div>
      {showInfoModal && (
        <ViewGroupChatRoomModal
          chatRoom={chatRoom!}
          onClose={() => setShowInfoModal(false)}
        />
      )}
    </div>
  )
}

export default ConversationPage
