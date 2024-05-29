import React, { FC, useState, useEffect, useRef, useCallback } from "react"
import { useGetUsersQuery } from "../features/users/usersApiSlice"
import { useGetContactsQuery } from "../features/contacts/contactsApiSlice"

import useDebounce from "../app/hooks/useDebounce"

import { RiSearch2Line } from "react-icons/ri"

import { Link } from "react-router-dom"

import { ContactSearchResult } from "../features/users/usersApiSlice"
import ProfileImage from "../components/ProfileImage"

interface ContactsPageProps {}

const ContactsPage: FC<ContactsPageProps> = ({}) => {
  const [searchTerm, setSearchTerm] = useState("")
  const [users, setUsers] = useState<ContactSearchResult[]>([])
  const [page, setPage] = useState(1)
  const [hasMore, setHasMore] = useState(true)
  const debouncedSearchTerm = useDebounce(searchTerm, 1000)
  const query = { searchTerm: debouncedSearchTerm, page }

  const {
    data: usersListResponse,
    error,
    isLoading,
  } = useGetUsersQuery(query, {
    skip: !debouncedSearchTerm,
  })

  const {
    data: contactsListResponse,
    error: contactsError,
    isLoading: contactsLoading,
  } = useGetContactsQuery(undefined, {
    skip: !!debouncedSearchTerm,
  })

  const observer = useRef<IntersectionObserver | null>(null)
  const lastUserRef = useCallback(
    (node: HTMLLIElement) => {
      if (isLoading) return
      if (observer.current) observer.current.disconnect()
      observer.current = new IntersectionObserver(entries => {
        if (entries[0].isIntersecting && hasMore) {
          setPage(prevPage => prevPage + 1)
        }
      })
      if (node) observer.current.observe(node)
    },
    [isLoading, hasMore],
  )

  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value
    setSearchTerm(value)
    setPage(1)
    setUsers([])
  }

  useEffect(() => {
    if (debouncedSearchTerm) {
      setSearchTerm(debouncedSearchTerm)
    }
  }, [debouncedSearchTerm])

  useEffect(() => {
    if (usersListResponse) {
      setUsers(prevUsers => [...prevUsers, ...usersListResponse.users])
      setHasMore(usersListResponse.page < usersListResponse.totalPages)
    }
  }, [usersListResponse])

  return (
    <div className="flex-1 bg-slate-900 text-white flex flex-col overflow-y-auto mb-20">
      <div className="bg-gray-800 py-3 px-5 flex items-center justify-center">
        <form className="w-full">
          <div className="flex flex-row w-full bg-gray-700 rounded-md overflow-hidden">
            <input
              type="text"
              placeholder="Search users..."
              className="px-3 py-2 flex-1 bg-transparent text-gray-100 focus:outline-none"
              value={searchTerm}
              onChange={handleSearch}
            />
            <button className="p-2 bg-gray-600 hover:bg-gray-500 transition-colors duration-300">
              <RiSearch2Line className="w-6 h-6 text-gray-100" />
            </button>
          </div>
        </form>
      </div>
      <div className="flex-1 overflow-y-auto">
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
        {contactsLoading && (
          <div className="flex justify-center items-center h-full">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-gray-500"></div>
          </div>
        )}
        {contactsError && (
          <div className="flex justify-center items-center h-full text-red-500">
            Error
          </div>
        )}
        {!debouncedSearchTerm && contactsListResponse && (
          <>
            <h2 className=" bg-gray-800 text-gray-100 px-5 py-3 uppercase font-bold tracking-widest">
              Contacts
            </h2>
            <ul className="divide-y divide-gray-700">
              {contactsListResponse.users.map((contact, index) => (
                <li
                  key={contact.id}
                  className="bg-gray-800 hover:bg-gray-700 transition-colors duration-300"
                >
                  <Link
                    to={`/contacts/${contact.id}`}
                    className="flex items-center px-5 py-4"
                  >
                    <div className="relative">
                      <ProfileImage src={contact.profilePictureUrl} size={12} />
                    </div>
                    <div className="flex-1 ml-5">
                      <h2 className="font-semibold text-gray-100">
                        {contact.displayName}
                      </h2>
                      <p className="text-gray-400 text-sm">Contact</p>
                    </div>
                  </Link>
                </li>
              ))}
            </ul>
          </>
        )}
        {users.length > 0 && (
          <ul className="divide-y divide-gray-700">
            {users.map((user, index) => {
              const isLastUser = index === users.length - 1
              return (
                <li
                  key={user.id}
                  className="bg-gray-800 hover:bg-gray-700 transition-colors duration-300"
                  ref={isLastUser ? lastUserRef : null}
                >
                  <Link
                    to={`/contacts/${user.id}`}
                    className="flex items-center px-5 py-4"
                  >
                    <div className="relative">
                      {user.profilePictureUrl ? (
                        <img
                          src={user.profilePictureUrl}
                          alt={user.displayName}
                          className="w-full h-full object-cover"
                        />
                      ) : (
                        <>
                          <div className="relative w-12 h-12 rounded-full overflow-hidden">
                            <img
                              src="https://austinpeopleworks.com/wp-content/uploads/2020/12/blank-profile-picture-973460_1280.png"
                              alt={user.displayName}
                              className="w-full h-full object-cover"
                            />
                          </div>
                        </>
                      )}
                    </div>
                    <div className="flex-1 ml-5">
                      <h2 className="font-semibold text-gray-100">
                        {user.displayName}
                      </h2>
                      {user.isContact && (
                        <p className="text-gray-400 text-sm">Contact</p>
                      )}
                    </div>
                  </Link>
                </li>
              )
            })}
          </ul>
        )}
      </div>
    </div>
  )
}
export default ContactsPage
