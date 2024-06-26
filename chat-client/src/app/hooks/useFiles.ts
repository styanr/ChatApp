import axios, { AxiosError } from "axios"
import { useState } from "react"

const useFiles = () => {
  const [isUploading, setIsUploading] = useState(false)
  const [uploadError, setUploadError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleAxiosError = (
    error: unknown,
    setErrorCallback: React.Dispatch<React.SetStateAction<string | null>>,
  ) => {
    if (axios.isAxiosError(error)) {
      const axiosError = error as AxiosError
      setErrorCallback(String(axiosError.response?.data))
    } else {
      setErrorCallback("An unexpected error occurred")
    }
  }

  const uploadFile = async (file: File) => {
    setIsUploading(true)
    setUploadError(null)

    try {
      const formData = new FormData()
      formData.append("file", file)

      const response = await axios.post("/api/files", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })

      return response.data
    } catch (error) {
      handleAxiosError(error, setUploadError)
    } finally {
      setIsUploading(false)
    }
  }

  const getFile = async (id: string) => {
    setIsLoading(true)
    setError(null)

    try {
      const response = await axios.get(`/api/files/${id}`, {
        responseType: "blob",
      })

      return response.data
    } catch (error) {
      handleAxiosError(error, setError)
    } finally {
      setIsLoading(false)
    }
  }

  return {
    uploadFile,
    getFile,
    isUploading,
    uploadError,
    isLoading,
    error,
  }
}

export default useFiles
