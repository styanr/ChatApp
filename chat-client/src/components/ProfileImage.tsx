import React, { FC, useState, useEffect } from "react"

import useProfilePictures from "../app/hooks/useProfilePictures"

interface ProfileImageProps {
  id: string | undefined
  size: number
}

const ProfileImage: FC<ProfileImageProps> = ({ id, size }) => {
  const imageFallback =
    "https://austinpeopleworks.com/wp-content/uploads/2020/12/blank-profile-picture-973460_1280.png"

  const handleError = (e: React.SyntheticEvent<HTMLImageElement, Event>) => {
    e.currentTarget.src = imageFallback
  }

  const { getProfilePicture, error, isLoading } = useProfilePictures()

  const [src, setSrc] = useState<string | null>(null)

  useEffect(() => {
    console.log("id updated", id)
    if (id) {
      getProfilePicture(id).then(data => {
        if (data) {
          console.log("data updated")
          console.log(error, isLoading)
          setSrc(URL.createObjectURL(data))
        }
      })
    }
  }, [id])

  return (
    <div
      className={`relative w-full aspect-square max-w-${size} max-h-${size} rounded-full overflow-hidden`}
    >
      {isLoading && (
        <div className="absolute inset-0 flex items-center justify-center bg-slate-700 text-white">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-gray-500"></div>
        </div>
      )}
      <img
        src={error ? imageFallback : src || imageFallback}
        alt="profile picture"
        className="w-full h-full rounded-full object-cover"
        onError={handleError}
      />
    </div>
  )
}

export default ProfileImage
