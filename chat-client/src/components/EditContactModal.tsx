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
      <div className="bg-ca-dark-gray rounded-lg shadow-lg p-6">
        <h2 className="text-xl font-bold mb-4">Edit Contact</h2>
        <input
          type="text"
          className="rounded-md px-3 py-2 w-full mb-4 bg-ca-dark text-white"
          value={newDisplayName}
          onChange={e => setNewDisplayName(e.target.value)}
        />
        <div className="flex justify-end">
          <button
            className="bg-ca-dark hover:bg-ca-gray text-white font-bold py-2 px-4 rounded mr-2"
            onClick={onCancel}
          >
            Cancel
          </button>
          <button
            className="bg-ca-blue hover:bg-ca-dark-blue text-white font-bold py-2 px-4 rounded"
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
