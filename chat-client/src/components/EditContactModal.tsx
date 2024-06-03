import { FC, useState } from "react"

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
    <div className="fixed inset-0 flex items-center justify-center z-50 bg-black bg-opacity-50 text-white">
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

export default EditContactModal
