import Link from "next/link";

interface ActionCardProps {
  to: string;
  title: string;
  icon?: React.ReactNode;
  description: string;
}

export default function ActionCard({
  to,
  title,
  icon,
  description,
}: ActionCardProps) {
  return (
    <Link href={to}>
      <div className="bg-bg border border-gray-300 border-2 rounded-md h-full w-full p-4 hover:bg-gray-50 hover:border-blue-500 hover:text-blue-500">
        <div className="flex mb-5">
          <div className="my-auto mx-[10px]">{icon && icon}</div>
          <h1 className="text-xl font-medium">{title}</h1>
        </div>
        <p className="mb-[8px]">{description}</p>
      </div>
    </Link>
  );
}
