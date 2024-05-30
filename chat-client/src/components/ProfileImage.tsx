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
    <div
      className={`relative w-full aspect-square max-w-${size} max-h-${size}`}
    >
      <img
        src={src ? src : imageFallback}
        alt="profile picture"
        className="w-full h-full rounded-full object-cover"
        onError={handleError}
      />
    </div>
  )
}

export default ProfileImage
