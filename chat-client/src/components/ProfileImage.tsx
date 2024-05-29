import React, { FC } from "react"

interface ProfileImageProps {
  src: string | undefined
  size: number
}

const ProfileImage: FC<ProfileImageProps> = ({ src, size }) => {
  const imageFallback =
    "https://austinpeopleworks.com/wp-content/uploads/2020/12/blank-profile-picture-973460_1280.png"

  const handleError = (e: React.SyntheticEvent<HTMLImageElement, Event>) => {
    e.currentTarget.src = imageFallback
  }

  return (
    <div>
      <img
        src={src ? src : imageFallback}
        alt="profile picture"
        className={`w-${size} h-${size} rounded-full`}
        onError={handleError}
      />
    </div>
  )
}

export default ProfileImage
