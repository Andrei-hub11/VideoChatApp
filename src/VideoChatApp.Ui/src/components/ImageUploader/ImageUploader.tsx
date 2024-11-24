import "./_ImageUploader.scss";

import ImageInputIcon from "../../assets/icons/ic_twotone-add-a-photo.svg";
import TrashIcon from "../../assets/icons/mdi_trash-outline.svg";

import { ImageUploaderProps } from "../../contracts";
import Image from "../Image/Image";

interface ImgUploaderProps {
  imgUploaderProps: ImageUploaderProps;
}

function ImageUploader({ imgUploaderProps }: ImgUploaderProps) {
  const {
    handleTrashClick,
    handleImageChange,
    handleImageInputClick,
    imagePreview,
    profileImageInputRef,
  } = imgUploaderProps;

  return (
    <div
      className={`image-uploader ${imagePreview ? "image-uploader--has-image" : ""}`}
      onClick={() => {
        if (imagePreview) {
          return;
        }
        handleImageInputClick();
      }}
    >
      <div
        className={`image-uploader__trash-icon ${imagePreview ? "image-uploader__trash-icon--visible" : ""}`}
        onClick={(e) => {
          e.stopPropagation();
          handleTrashClick();
        }}
      >
        <Image
          {...{
            props: {
              src: TrashIcon,
              alt: "trash icon",
              height: 24,
              width: 24,
            },
          }}
        />
      </div>
      <img
        src={imagePreview ? imagePreview : ImageInputIcon}
        alt="profile image"
      />
      <input
        ref={profileImageInputRef}
        onChange={handleImageChange}
        type="file"
        accept="image/png, image/jpeg, image/webp"
      />
    </div>
  );
}

export default ImageUploader;
