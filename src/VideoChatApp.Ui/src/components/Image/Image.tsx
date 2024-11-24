import { ImageProps } from "../../contracts";

interface ImgProps {
  props: ImageProps;
}

function Image({ props }: ImgProps) {
  const { src, alt, height, width, style } = props;

  return (
    <img style={style} src={src} alt={alt} height={height} width={width} />
  );
}

export default Image;
