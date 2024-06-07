import React, { FC } from "react"
import { RiUploadCloudFill } from "react-icons/ri"
import useProfilePictures from "../app/hooks/useProfilePictures"
import ProfileImage from "./ProfileImage"

interface ImageUploadProps {
  profilePictureId: string | undefined
  onImageUpload: (imageId: string) => void
}

const ImageUpload: FC<ImageUploadProps> = ({
  profilePictureId,
  onImageUpload,
}) => {
  const { uploadProfilePicture } = useProfilePictures()

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      const file = e.target.files[0]
      const data = await uploadProfilePicture(file)
      console.log(data)
      onImageUpload(data)
    }
  }

  return (
    <div>
      <label
        htmlFor="profilePicture"
        className="flex justify-center mb-4 cursor-pointer rounded-full aspect-square h-48 w-48 overflow-hidden relative"
      >
        <ProfileImage id={profilePictureId} size={48} />
        <div className="absolute inset-0 items-center justify-center opacity-0 bg-opacity-50 flex hover:opacity-100 bg-black flex-col text-white transition-all duration-300">
          <RiUploadCloudFill className="text-4xl" />
          <p>Upload Picture</p>
        </div>
      </label>
      <input
        id="profilePicture"
        name="profilePicture"
        type="file"
        className="hidden"
        onChange={handleFileChange}
      />
    </div>
  )
}

export default ImageUpload
