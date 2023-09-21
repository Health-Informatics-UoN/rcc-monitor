import * as React from "react";
import { css, cva } from "@/styled-system/css";

const card = cva({
  base: {
    rounded: "0.5rem",
    borderWidth: "1px",
    borderStyle: "solid",
    borderColor: "cardForeground",
    backgroundColor: "card",
    color: "cardForeground",
    shadow: "sm",
  },
});

const Card = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div ref={ref} className={card({})} {...props} />
));
Card.displayName = "Card";

const CardHeader = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div
    ref={ref}
    className={css({
      padding: "1.5rem",
      marginTop: "0.25rem",
      flex: "auto",
      flexDirection: "column",
    })}
    {...props}
  />
));
CardHeader.displayName = "CardHeader";

const CardTitle = React.forwardRef<
  HTMLParagraphElement,
  React.HTMLAttributes<HTMLHeadingElement>
>(({ className, ...props }, ref) => (
  <h3
    ref={ref}
    className={css({
      fontWeight: "semibold",
      lineHeight: 1,
      letterSpacing: "-0.025em",
      fontSize: "1.5rem",
    })}
    {...props}
  />
));
CardTitle.displayName = "CardTitle";

const CardDescription = React.forwardRef<
  HTMLParagraphElement,
  React.HTMLAttributes<HTMLParagraphElement>
>(({ className, ...props }, ref) => (
  <p
    ref={ref}
    className={css({
      fontSize: "0.875rem",
    })}
    {...props}
  />
));
CardDescription.displayName = "CardDescription";

const CardContent = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div
    ref={ref}
    className={css({
      padding: "1.5rem",
      paddingTop: "0",
    })}
    {...props}
  />
));
CardContent.displayName = "CardContent";

const CardFooter = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div
    ref={ref}
    className={css({
      flex: "auto",
      alignItems: "center",
      padding: "1.5rem",
      paddingTop: "0",
    })}
    {...props}
  />
));
CardFooter.displayName = "CardFooter";

export {
  Card,
  CardHeader,
  CardFooter,
  CardTitle,
  CardDescription,
  CardContent,
};
