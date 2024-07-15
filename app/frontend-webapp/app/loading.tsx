/**
 * Base loading state for the entire app.
 */
export default function Loading() {
  return (
    <div className="flex justify-center items-center h-screen">
      <div
        className="animate-spin rounded-full h-32 w-32 border-t-2 border-b-2"
        style={{ transform: "rotate(360deg)" }}
      ></div>
    </div>
  );
}
