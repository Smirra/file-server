export default function Page() {
  return (
    <div className="max-w-[48em] mx-2">
      <h1 className="text-3xl">Greetings!</h1><br />
      <p className="text-lg">
        This is a live demo of a very simple file hosting service app (think iCloud or OneDrive) with a frontend written in Next.js with some .NET Core microservice APIs working under the hood. It is deployed on Azure Kubernetes Service (AKS) using GitHub Actions.
        <br /><br />
        The source code can be found on <a href="https://github.com/smirra/file-server/">GitHub</a>.
      </p>
    </div>
  )
}