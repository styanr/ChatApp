import React, { FC } from "react"
import ProfileImage from "../components/ProfileImage"
import { Menu, MenuItem, MenuButton } from "@szhsin/react-menu"
import {
  RiDeleteBin6Fill,
  RiEdit2Fill,
  RiLogoutBoxFill,
  RiMore2Fill,
} from "react-icons/ri"

interface ContactDisplayProps {
  data: any
  isCurrentUser: boolean
  onEdit?: () => void
  onDelete?: () => void
  onAdd?: () => void
  onLogout?: () => void
}

const ContactDisplay: FC<ContactDisplayProps> = ({
  data,
  isCurrentUser,
  onEdit,
  onDelete,
  onAdd,
  onLogout,
}) => {
  return (
    <div className="flex-1 bg-slate-800 text-white flex overflow-y-auto mb-20">
      <div className="flex flex-col w-full">
        <div className="flex flex-row items-center justify-between p-6">
          <div className="flex flex-row items-center">
            <ProfileImage src={data?.profilePictureUrl} size={16} />
            <div className="ml-4">
              <h2 className="text-xl font-bold">{data?.displayName}</h2>
              {!isCurrentUser && (
                <p className="text-gray-400">
                  {data?.isContact ? "Contact" : "Not a contact"}
                </p>
              )}
            </div>
          </div>
          <Menu
            menuButton={
              <MenuButton>
                <RiMore2Fill className="w-6 h-6" />
              </MenuButton>
            }
            menuClassName="box-border z-50 text-sm bg-gray-800 py-3 border rounded-md shadow-lg select-none focus:outline-none min-w-[9rem] border-none w-48"
          >
            <MenuItem className="px-3 py-3 focus:bg-gray-700" onClick={onEdit}>
              <div className="flex items-center flex-row">
                <RiEdit2Fill className="w-6 h-6 pr-2" />
                Edit
              </div>
            </MenuItem>
            {isCurrentUser ? (
              <MenuItem
                className="px-3 py-3 focus:bg-gray-700"
                onClick={onLogout}
              >
                <div className="flex items-center flex-row">
                  <RiLogoutBoxFill className="w-6 h-6 pr-2" />
                  Log out
                </div>
              </MenuItem>
            ) : (
              <>
                {data?.isContact ? (
                  <>
                    <MenuItem
                      className="px-3 py-3 focus:bg-gray-700"
                      onClick={onDelete}
                    >
                      <div className="flex items-center flex-row">
                        <RiDeleteBin6Fill className="w-6 h-6 pr-2" />
                        Remove
                      </div>
                    </MenuItem>
                  </>
                ) : (
                  <MenuItem
                    className="px-3 py-3 focus:bg-gray-700"
                    onClick={onAdd}
                  >
                    Add contact
                  </MenuItem>
                )}
              </>
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
    </div>
  )
}

const InfoCard: FC<{ title: string; value: string }> = ({ title, value }) => {
  return (
    <div className="flex flex-col w-full bg-slate-800 p-6 rounded-lg overflow-hidden shadow-lg">
      <h2 className="text-lg font-bold mb-1">{title}</h2>
      <p className="">{value}</p>
    </div>
  )
}

export default ContactDisplay
