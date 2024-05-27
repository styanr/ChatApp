import React, { FC } from "react"
import { Link } from "react-router-dom"

interface PublicProps {}

const Public: FC<PublicProps> = ({}) => {
  return (
    <div>
      You are not logged in. <Link to="/login">Login</Link>
    </div>
  )
}

export default Public
