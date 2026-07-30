// Auth
export interface LoginDTO {
  email: string
  password: string
}

export interface RegisterDTO {
  email: string
  password: string
  username: string
}

export interface ForgotPasswordDTO {
  email: string
}

export interface ResetPasswordDTO {
  email: string
  token: string
  newPassword: string
}

// Hero
export interface HeroDTO {
  name: string | null
  title: string
  subtitle: string | null
  imageUrl: string | null
}

export interface HeroUpdateDTO {
  name?: string
  title: string
  subtitle?: string
  imageUrl?: string
}

// About
export interface AboutDTO {
  title: string
  body: string
  imageUrl: string | null
  tagline: string | null
}

export interface AboutUpdateDTO {
  title: string
  body: string
  imageUrl?: string
  tagline?: string
}

// Experience
export interface ExperienceDTO {
  id: string
  title: string
  organization: string
  type: string
  startDate: string
  endDate: string | null
  location: string | null
  description: string | null
}

export interface ExperienceCreateDTO {
  title: string
  organization: string
  type: string
  startDate: string
  endDate?: string
  location?: string
  description?: string
}

export type ExperienceUpdateDTO = ExperienceCreateDTO

// Project
export interface ProjectDTO {
  id: string
  title: string
  slug: string
  description: string
  imageUrl: string | null
  liveUrl: string | null
  githubUrl: string | null
  stack: string
  isFeatured: boolean
}

export interface ProjectCreateDTO {
  title: string
  slug: string
  description: string
  imageUrl?: string
  liveUrl?: string
  githubUrl?: string
  stack: string
  isFeatured: boolean
}

export type ProjectUpdateDTO = Omit<ProjectCreateDTO, 'slug'>

// Social Link
export interface SocialLinkDTO {
  id: string
  platform: string
  url: string
}

export interface SocialLinkCreateDTO {
  platform: string
  url: string
}

export type SocialLinkUpdateDTO = SocialLinkCreateDTO

// Dashboard
export interface SectionStatusDTO {
  isConfigured: boolean
  updatedAt: string | null
}

export interface MultiSectionStatusDTO {
  count: number
  lastUpdatedAt: string | null
}

export interface DashboardDTO {
  hero: SectionStatusDTO
  about: SectionStatusDTO
  experience: MultiSectionStatusDTO
  projects: MultiSectionStatusDTO
  socialLinks: MultiSectionStatusDTO
}