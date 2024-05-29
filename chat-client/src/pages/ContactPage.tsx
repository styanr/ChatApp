import React, { FC, useState } from "react"
import { useParams } from "react-router-dom"
import { useGetUserByIdQuery } from "../features/users/usersApiSlice"
import {
  useAddContactMutation,
  useUpdateContactMutation,
  useDeleteContactMutation,
} from "../features/contacts/contactsApiSlice"

import ProfileImage from "../components/ProfileImage"

import { Menu, MenuItem, MenuButton } from "@szhsin/react-menu"

import { RiDeleteBin6Fill, RiEdit2Fill, RiMore2Fill } from "react-icons/ri"

interface ContactPageProps {}

const BaseContactPage: FC<{ children: React.ReactNode }> = ({ children }) => {
  return (
    <div className="flex-1 bg-slate-800 text-white flex overflow-y-auto mb-20">
      {children}
    </div>
  )
}

const InfoCard: FC<{ title: string; value: string }> = ({ title, value }) => {
  return (
    <div className="flex flex-col w-full bg-slate-800 p-6 rounded-lg overflow-scroll">
      <h2 className="text-lg font-bold mb-1">{title}</h2>
      <p className="">{value}</p>
    </div>
  )
}

interface EditContactModalProps {
  currentDisplayName: string
  onSave: (newDisplayName: string) => void
  onCancel: () => void
}

const EditContactModal: FC<EditContactModalProps> = ({
  currentDisplayName,
  onSave,
  onCancel,
}) => {
  const [newDisplayName, setNewDisplayName] = useState(currentDisplayName)

  const handleSave = () => {
    onSave(newDisplayName)
  }

  return (
    <div className="fixed inset-0 flex items-center justify-center z-50 bg-black bg-opacity-50">
      <div className="bg-slate-800 rounded-lg shadow-lg p-6">
        <h2 className="text-xl font-bold mb-4">Edit Contact</h2>
        <input
          type="text"
          className="rounded-md px-3 py-2 w-full mb-4 bg-gray-900 text-white"
          value={newDisplayName}
          onChange={e => setNewDisplayName(e.target.value)}
        />
        <div className="flex justify-end">
          <button
            className="bg-gray-900 hover:bg-gray-800 text-white font-bold py-2 px-4 rounded mr-2"
            onClick={onCancel}
          >
            Cancel
          </button>
          <button
            className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
            onClick={handleSave}
          >
            Save
          </button>
        </div>
      </div>
    </div>
  )
}

const ContactPage: FC<ContactPageProps> = ({}) => {
  const { id } = useParams()

  const { data, error, isLoading } = useGetUserByIdQuery(id as string)
  const [addContact, { error: addContactError }] = useAddContactMutation()
  const [updateContact, { error: updateContactError }] =
    useUpdateContactMutation()
  const [deleteContact, { error: deleteContactError }] =
    useDeleteContactMutation()

  const handleAddContact = () => {
    addContact(id as string)
  }

  const [showEditModal, setShowEditModal] = useState(false)

  const handleUpdateContact = (newDisplayName: string) => {
    updateContact({ contactUserId: id as string, displayName: newDisplayName })
    setShowEditModal(false)
  }

  const handleDeleteContact = () => {
    deleteContact(id as string)
  }

  if (isLoading)
    return (
      <div className="flex justify-center items-center h-full bg-slate-800">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-gray-500"></div>
      </div>
    )

  if (error) {
    console.error(error)
    return (
      <BaseContactPage>
        Error: {"message" in error ? error.message : "An error occurred"}
      </BaseContactPage>
    )
  }

  return (
    <BaseContactPage>
      {showEditModal && (
        <EditContactModal
          currentDisplayName={data?.displayName || ""}
          onSave={handleUpdateContact}
          onCancel={() => setShowEditModal(false)}
        />
      )}
      <div className="flex flex-col w-full">
        <div className="flex flex-row items-center justify-between p-6">
          <div className="flex flex-row items-center">
            <ProfileImage src={data?.profilePictureUrl} size={16} />
            <div className="ml-4">
              <h2 className="text-xl font-bold">{data?.displayName}</h2>
              <p className="text-gray-400">
                {data?.isContact ? "Contact" : "Not a contact"}
              </p>
            </div>
          </div>
          <Menu
            menuButton={
              <MenuButton>
                <RiMore2Fill className="w-6 h-6" />
              </MenuButton>
            }
            menuClassName="box-border z-50 text-sm bg-gray-800 py-3 border rounded-md shadow-lg select-none focus:outline-none min-w-[9rem] border-none w-48"
            onChange={console.log}
          >
            {data?.isContact ? (
              <>
                <MenuItem
                  className="px-3 py-3 focus:bg-gray-700"
                  onClick={() => setShowEditModal(true)}
                >
                  <div className="flex items-center flex-row">
                    <RiEdit2Fill className="w-6 h-6 pr-2" />
                    Edit contact
                  </div>
                </MenuItem>

                <MenuItem
                  className="px-3 py-3 focus:bg-gray-700"
                  onClick={handleDeleteContact}
                >
                  <div className="flex items-center flex-row">
                    <RiDeleteBin6Fill className="w-6 h-6 pr-2" />
                    Remove contact
                  </div>
                </MenuItem>
              </>
            ) : (
              <MenuItem
                className="px-3 py-3 focus:bg-gray-700"
                onClick={handleAddContact}
              >
                Add contact
              </MenuItem>
            )}
          </Menu>
        </div>
        <div className="flex-1 bg-gray-900 p-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <InfoCard title="Bio" value={data?.bio || "N/A"} />
            <InfoCard title="Handle" value={data?.handle || "N/A"} />
          </div>
        </div>
      </div>
    </BaseContactPage>
  )
}

export default ContactPage
