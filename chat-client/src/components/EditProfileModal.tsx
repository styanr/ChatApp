import React, { FC, useState } from "react"
import {
  ContactSearchResult,
  UserUpdate,
} from "../features/users/usersApiSlice"

interface EditProfileModalProps {
  user: ContactSearchResult
  onSave: (user: UserUpdate) => void
  onCancel: () => void
}

const EditProfileModal: FC<EditProfileModalProps> = ({
  user,
  onSave,
  onCancel,
}) => {
  const [userUpdate, setUserUpdate] = useState<UserUpdate>(user)

  const handleSave = () => {
    onSave(userUpdate)
  }
  return (
    <div className="fixed inset-0 flex items-center justify-center z-50 bg-black bg-opacity-50 text-white">
      <div className="bg-slate-800 rounded-lg shadow-lg p-6">
        <h2 className="text-xl font-bold mb-4">Edit Contact</h2>

        <label
          className="block text-sm font-medium text-gray-400 mb-2"
          htmlFor="profilePictureUrl"
        >
          Profile Picture URL
        </label>
        <input
          id="profilePictureUrl"
          name="profilePictureUrl"
          type="text"
          className="rounded-md px-3 py-2 w-full mb-4 bg-gray-900 text-white"
          value={userUpdate.profilePictureUrl || ""}
          onChange={e =>
            setUserUpdate({ ...userUpdate, profilePictureUrl: e.target.value })
          }
        />

        <label
          className="block text-sm font-medium text-gray-400 mb-2"
          htmlFor="displayName"
        >
          Display Name
        </label>
        <input
          id="displayName"
          name="displayName"
          type="text"
          className="rounded-md px-3 py-2 w-full mb-4 bg-gray-900 text-white"
          value={userUpdate.displayName}
          onChange={e =>
            setUserUpdate({ ...userUpdate, displayName: e.target.value })
          }
        />

        <label
          className="block text-sm font-medium text-gray-400 mb-2"
          htmlFor="handle"
        >
          Handle
        </label>
        <input
          id="handle"
          name="handle"
          type="text"
          className="rounded-md px-3 py-2 w-full mb-4 bg-gray-900 text-white"
          value={userUpdate.handle || ""}
          onChange={e =>
            setUserUpdate({ ...userUpdate, handle: e.target.value })
          }
        />

        <label
          className="block text-sm font-medium text-gray-400 mb-2"
          htmlFor="bio"
        >
          Bio
        </label>

        <textarea
          id="bio"
          name="bio"
          className="rounded-md px-3 py-2 w-full mb-4 bg-gray-900 text-white"
          value={userUpdate.bio || ""}
          onChange={e => setUserUpdate({ ...userUpdate, bio: e.target.value })}
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

export default EditProfileModal