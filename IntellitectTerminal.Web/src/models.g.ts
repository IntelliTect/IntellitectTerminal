import * as metadata from './metadata.g'
import { Model, DataSource, convertToModel, mapToModel } from 'coalesce-vue/lib/model'

export enum CompilationLanguages {
  None = 0,
  Python = 1,
}


export interface Challenge extends Model<typeof metadata.Challenge> {
  challengeId: number | null
  question: string | null
  level: number | null
  compilationLanguage: CompilationLanguages | null
  answer: string | null
}
export class Challenge {
  
  /** Mutates the input object and its descendents into a valid Challenge implementation. */
  static convert(data?: Partial<Challenge>): Challenge {
    return convertToModel(data || {}, metadata.Challenge) 
  }
  
  /** Maps the input object and its descendents to a new, valid Challenge implementation. */
  static map(data?: Partial<Challenge>): Challenge {
    return mapToModel(data || {}, metadata.Challenge) 
  }
  
  /** Instantiate a new Challenge, optionally basing it on the given data. */
  constructor(data?: Partial<Challenge> | {[k: string]: any}) {
      Object.assign(this, Challenge.map(data || {}));
  }
}


export interface Submission extends Model<typeof metadata.Submission> {
  submissionId: number | null
  user: User | null
  challenge: Challenge | null
  isCorrect: boolean | null
  content: string | null
}
export class Submission {
  
  /** Mutates the input object and its descendents into a valid Submission implementation. */
  static convert(data?: Partial<Submission>): Submission {
    return convertToModel(data || {}, metadata.Submission) 
  }
  
  /** Maps the input object and its descendents to a new, valid Submission implementation. */
  static map(data?: Partial<Submission>): Submission {
    return mapToModel(data || {}, metadata.Submission) 
  }
  
  /** Instantiate a new Submission, optionally basing it on the given data. */
  constructor(data?: Partial<Submission> | {[k: string]: any}) {
      Object.assign(this, Submission.map(data || {}));
  }
}


export interface User extends Model<typeof metadata.User> {
  userId: string | null
  fileSystem: string | null
  creationTime: Date | null
}
export class User {
  
  /** Mutates the input object and its descendents into a valid User implementation. */
  static convert(data?: Partial<User>): User {
    return convertToModel(data || {}, metadata.User) 
  }
  
  /** Maps the input object and its descendents to a new, valid User implementation. */
  static map(data?: Partial<User>): User {
    return mapToModel(data || {}, metadata.User) 
  }
  
  /** Instantiate a new User, optionally basing it on the given data. */
  constructor(data?: Partial<User> | {[k: string]: any}) {
      Object.assign(this, User.map(data || {}));
  }
}


