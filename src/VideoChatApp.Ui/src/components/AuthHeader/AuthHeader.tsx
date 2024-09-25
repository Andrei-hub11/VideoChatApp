import "./_AuthHeader.scss";

interface AuthHeaderProps {
  title: React.ReactNode;
  subtitle: string;
  description: string;
}

function AuthHeader({ title, subtitle, description }: AuthHeaderProps) {
  return (
    <div className="auth-header">
      <h1 className="auth-header__title">{title}</h1>
      <div className="auth-header__container">
        <h2 className="auth-header__subtitle">{subtitle}</h2>
        <p className="auth-header__description">{description}</p>
      </div>
    </div>
  );
}

export default AuthHeader;
