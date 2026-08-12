import {
  isRouteErrorResponse,
  Links,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
} from "react-router";
import "./root.css";

import "@mantine/core/styles.css";
import type { Route } from "./+types/root";
import {
  MantineProvider,
  createTheme,
  type MantineColorsTuple,
