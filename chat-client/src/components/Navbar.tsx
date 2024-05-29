import React from "react"
import { NavLink } from "react-router-dom"
import {
  RiMessage3Fill,
  RiMessage3Line,
  RiContactsBook2Fill,
  RiContactsBook2Line,
  RiCriminalFill,
  RiCriminalLine,
} from "react-icons/ri"

interface NavBarItem {
  name: string
  link: string
  iconActive?: React.ReactNode
  iconInactive?: React.ReactNode
}

const Links: NavBarItem[] = [
  {
    name: "Messages",
    link: "/messages",
    iconActive: <RiMessage3Fill className="w-6 h-6" />,
    iconInactive: <RiMessage3Line className="w-6 h-6" />,
  },
  {
    name: "Contacts",
    link: "/contacts",
    iconActive: <RiContactsBook2Fill className="w-6 h-6" />,
    iconInactive: <RiContactsBook2Line className="w-6 h-6" />,
  },
  {
    name: "Me",
    link: "/me",
    iconActive: <RiCriminalFill className="w-6 h-6" />,
    iconInactive: <RiCriminalLine className="w-6 h-6" />,
  },
]

const Navbar: React.FC = () => {
  return (
    <div className="h-20 w-full bg-slate-950 fixed bottom-0 left-0 right-0">
      <nav className="h-full">
        <ul className="flex justify-around items-center h-full">
          {Links.map(link => (
            <li key={link.name}>
              <NavLink
                to={link.link}
                className="flex flex-col items-center justify-center h-full text-slate-100 
                     hover:bg-slate-800 focus:bg-slate-800 transition-colors duration-300 ease-in-out p-3 rounded-xl"
              >
                {({ isActive }) => (
                  <>
                    {isActive ? link.iconActive : link.iconInactive}
                    <span className="text-xs">{link.name}</span>
                  </>
                )}
              </NavLink>
            </li>
          ))}
        </ul>
      </nav>
    </div>
  )
}

export default Navbar
