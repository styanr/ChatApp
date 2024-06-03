import React, { FC, useState } from "react";
import { useParams, Navigate } from "react-router-dom";
import {
  useGetUserByIdQuery,
  useGetCurrentUserQuery,
} from "../features/users/usersApiSlice";
import {
  useAddContactMutation,
  useUpdateContactMutation,
  useDeleteContactMutation,
} from "../features/contacts/contactsApiSlice";
import ContactDisplay from "../components/ContactDisplay";
import EditContactModal from "../components/EditContactModal";

const ContactPage: FC = () => {
  const { id } = useParams();
  const { data, error, isLoading } = useGetUserByIdQuery(id as string);
  const [addContact] = useAddContactMutation();
  const [updateContact] = useUpdateContactMutation();
  const [deleteContact] = useDeleteContactMutation();
  const { data: currentUser } = useGetCurrentUserQuery();
  const [showEditModal, setShowEditModal] = useState(false);

  const handleAddContact = () => addContact(id as string);
  const handleUpdateContact = (newDisplayName: string) => {
    updateContact({ contactUserId: id as string, displayName: newDisplayName });
    setShowEditModal(false);
  };
  const handleDeleteContact = () => deleteContact(id as string);

  if (isLoading)
    return (
      <div className="flex justify-center items-center h-full bg-slate-800">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-gray-500"></div>
      </div>
    );

  if (error) {
    console.error(error);
    return <div>Error: {"message" in error ? error.message : "An error occurred"}</div>;
  }

  if (currentUser?.id === id) {
    return <Navigate to="/me" />;
  }

  return (
    <>
      {showEditModal && (
        <EditContactModal
          currentDisplayName={data?.displayName || ""}
          onSave={handleUpdateContact}
          onCancel={() => setShowEditModal(false)}
        />
      )}
      <ContactDisplay
        data={data}
        isCurrentUser={false}
        onEdit={() => setShowEditModal(true)}
        onDelete={handleDeleteContact}
        onAdd={handleAddContact}
      />
    </>
  );
};

export default ContactPage;
