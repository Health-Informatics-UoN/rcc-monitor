import { css } from '@/styled-system/css';

export default function Home() {
  const x = '';
  return (
    <>
      <h2
        className={css({
          m: '10px',
          p: '10px',
          fontSize: '30px',
          fontWeight: '500',
        })}
      >
        Welcome.
      </h2>
    </>
  );
}
