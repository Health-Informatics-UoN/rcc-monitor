export default function Spinner() {
  return (
    <div className="flex justify-center items-center h-full mx-auto my-5">
      <div
        className="border-t-[3px] border-t-solid border-t-black rounded-full w-[30px] h-[30px] animate-spin"
        style={{
          border: "3px solid rgba(0, 0, 0, 0.3)",
        }}
      ></div>
    </div>
  );
}
